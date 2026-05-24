package com.demo.student_management.entity;

import jakarta.persistence.*;
import lombok.*;

@Entity
@Table(name = "PHANCONG_GIANGDAY")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
@EqualsAndHashCode(onlyExplicitlyIncluded = true)
public class PhanCongGiangDay {

    @Id
    @Column(name = "ID_PHANCONG", length = 36, nullable = false)
    @EqualsAndHashCode.Include
    private String idPhanCong;

    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "ID_GIAOVIEN", nullable = false)
    private GiaoVien giaoVien;

    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "ID_LOP", nullable = false)
    private Lop lop;

    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "ID_MONHOC", nullable = false)
    private MonHoc monHoc;

    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "ID_HOCKY", nullable = false)
    private HocKy hocKy;
}
