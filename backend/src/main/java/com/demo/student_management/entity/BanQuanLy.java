package com.demo.student_management.entity;

import jakarta.persistence.*;
import lombok.*;

@Entity
@Table(name = "BANQUANLY")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
@EqualsAndHashCode(onlyExplicitlyIncluded = true)
public class BanQuanLy {

    @Id
    @Column(name = "ID_BANQL", length = 20, nullable = false)
    @EqualsAndHashCode.Include
    private String idBanQl;

    @OneToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "ID_TAIKHOAN", nullable = false, unique = true)
    private TaiKhoan taiKhoan;
}